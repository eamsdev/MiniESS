import { ControllerDictionary } from '../router/RouterModels';
import { pageStore } from './AppStore';
import { routeLinks } from './Routes';

export const appController: ControllerDictionary = {
  [routeLinks.defaultRoute.name]: async () => {
    pageStore.homeRouted();
  },
  [routeLinks.loginRoute.name]: async () => {
    pageStore.loginRouted();
  },
};
