import { appController } from './AppController';
import { createRouterStore } from '../router/RouterModels';
import { pageStore } from './AppStore';
import { routeLinks } from './Routes';
import { todoUiStore } from './Todo/UiStore';

export const rootStore = {
  pageStore: pageStore,
  routerStore: createRouterStore(routeLinks, appController),
  todoStore: todoUiStore,
};
