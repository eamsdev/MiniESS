import { appController } from './AppController';
import { createRouterStore } from '../router/RouterModels';
import { pageStore } from './AppStore';
import { routeLinks } from './Routes';

export const rootStore = {
  pageStore: pageStore,
  routerStore: createRouterStore(routeLinks, appController),
};
