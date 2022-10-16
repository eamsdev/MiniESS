import { Route } from 'router5';
import { RouterStore } from './RouterStore';

export type onRoute = () => Promise<void>;

export type ControllerDictionary = {
  [key: string]: onRoute;
};

export type RouteDictionary = {
  defaultRoute: RouteRecord<Record<string, string>>;
} & {
  [key: string]: RouteRecord<Record<string, string>>;
};

export interface RouteWithBehaviour extends Route {
  name: string;
  path: string;
  onRoute?: onRoute;
}

export class RouteRecord<T extends Record<string, string>> {
  name: string;
  path: string;
  store?: RouterStore;

  constructor(name: string, path: string) {
    this.name = name;
    this.path = path;
    this.navigate.bind(this);
  }

  async navigate(params?: T) {
    await this.store?.navigate(this.name, params);
  }
}

export function createRouterStore(routeLinks: RouteDictionary, controller: ControllerDictionary) {
  return new RouterStore(routeLinks, controller);
}
