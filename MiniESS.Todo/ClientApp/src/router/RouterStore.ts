import { RouterFactory } from './RouterFactory';
import { RouteWithBehaviour, onRoute, ControllerDictionary, RouteDictionary } from './RouterModels';
import { observable, makeObservable, IReactionDisposer, reaction, action, runInAction } from 'mobx';
import { Router, State, transitionPath } from 'router5';

export class RouterStore {
  router: Router = null;
  routeReactor: IReactionDisposer;
  activeRoutes: RouteWithBehaviour[] = [];
  transitionRoutes: RouteWithBehaviour[] = [];
  routeDictionary: RouteDictionary = null;

  @observable.ref route: RouteWithBehaviour = null;
  @observable routerParameters: { [key: string]: string } = {};

  constructor(routeLinks: RouteDictionary, controller: ControllerDictionary) {
    makeObservable(this);
    this.routeDictionary = routeLinks;
    this.routeReactor = reaction(() => this.route, this.onRouteUpdated);
    this.activeRoutes = RouterStore.createActiveRoutes(this.toActionRoute(routeLinks), controller);
    this.router = RouterFactory.create(this);
    this.assignStores(routeLinks);
  }

  @action.bound
  onTransitionError(_: State, previous: State) {
    const previousRoute = this.activeRoutes.find((x) => x.name === previous.name);
    runInAction(() => {
      if (previous && !!previousRoute) {
        this.routerParameters = previous.params || {};
        this.route = previousRoute;
      } else {
        this.routeDictionary.defaultRoute.navigate();
      }
    });
  }

  @action.bound
  onTransitionSuccess(next: State, previous: State) {
    const nextRoute = this.findRoute(next.name);
    if (next && !!nextRoute) {
      runInAction(() => {
        this.transitionRoutes = transitionPath(next, previous || null).toActivate.map((x) =>
          this.findRoute(x),
        );
        this.routerParameters = next.params || {};
        this.route = nextRoute;
      });
    }
  }

  findRoute = (routeName: string) => {
    return this.activeRoutes.find((x) => x.name === routeName);
  };

  onRouteUpdated = async () => {
    await this.route.onRoute?.();
  };

  assignStores(routeLinks: RouteDictionary) {
    Object.keys(routeLinks).forEach((x) => {
      routeLinks[x].withStore(this);
    });
  }

  toActionRoute(routeLinks): RouteWithBehaviour[] {
    return Object.keys(routeLinks).map((x) => {
      return {
        path: routeLinks[x].path,
        name: routeLinks[x].name,
      };
    });
  }

  urlFor(routeName: string, routeParams?: Record<string, string>) {
    return this.router.buildUrl(routeName, routeParams);
  }

  navigate = (routeName: string, routeParams?: Record<string, string>) => {
    return new Promise((resolve, reject) => {
      this.router.navigate(routeName, routeParams || {}, (err?: any, state?: State) => {
        if (err) {
          reject(err);
        } else {
          resolve(state);
        }
      });
    });
  };

  static createActiveRoutes(routes: RouteWithBehaviour[], controller: ControllerDictionary) {
    return routes.map((route) => {
      const onRouteBehaviour = controller[route.name];
      if (!onRouteBehaviour) {
        throw Error(`Missing route activation behaviour: ${route.name}`);
      } else {
        route.onRoute = onRouteBehaviour;
      }

      return route;
    });
  }
}
