import createRouter, { Router, State } from 'router5';
import browserPlugin from 'router5-plugin-browser';
import { RouterStore } from './RouterStore';

export class RouterFactory {
  static create(store: RouterStore): Router {
    const router = createRouter(store.activeRoutes, { queryParamsMode: 'loose' });

    const makeMobxRouterPlugin = () => {
      return {
        onTransitionError(next: State, previous: State) {
          store.onTransitionError(next, previous);
        },
        onTransitionSuccess(next: State, previous: State) {
          store.onTransitionSuccess(next, previous);
        },
      };
    };

    router.usePlugin(browserPlugin({ useHash: true }), () => makeMobxRouterPlugin());

    return router;
  }
}
