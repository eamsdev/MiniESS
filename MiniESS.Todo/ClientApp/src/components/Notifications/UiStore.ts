import { NotificationModel } from './Models';
import { action, computed, makeObservable, observable } from 'mobx';
import { v4 as uuidv4 } from 'uuid';

export class UiStore {
  @observable notifs: NotificationModel[] = [];

  constructor() {
    makeObservable(this);
  }

  @computed
  get notifications() {
    return this.notifs;
  }

  @action
  addNotification(message: string, isSuccess = true) {
    const uuid = uuidv4();
    this.notifs.push(new NotificationModel(uuid, message, isSuccess));
    setTimeout(() => {
      this.closeNotif(uuid);
    }, 3000);
  }
  @action
  closeNotif(uuid: string) {
    // TODO: support fade transition
    this.notifs = this.notifs.filter((x) => x.uuid !== uuid);
  }
}

const notificationsUiStore = new UiStore();
export { notificationsUiStore };
