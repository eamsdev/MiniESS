import { action, computed, makeObservable, observable } from 'mobx';
import { routeLinks } from './Routes';

export enum Page {
  Home = 'home',
  Login = 'login',
}

export class AppStore {
  @observable activePage: Page = Page.Home;

  constructor() {
    makeObservable(this);
  }

  @computed
  get isAtHome() {
    return this.activePage === Page.Home;
  }

  @computed
  get isAtLogin() {
    return this.activePage === Page.Login;
  }

  @action
  async homeButtonClicked() {
    return !pageStore.isAtHome && (await routeLinks.defaultRoute.navigate());
  }

  @action
  async loginButtonClicked() {
    return !pageStore.isAtLogin && (await routeLinks.loginRoute.navigate());
  }

  @action
  homeRouted() {
    this.activePage = Page.Home;
  }

  @action
  loginRouted() {
    this.activePage = Page.Login;
  }
}

const pageStore = new AppStore();
export { pageStore };
