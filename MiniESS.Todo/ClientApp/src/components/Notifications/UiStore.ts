import { NotificationModel } from './Models';
import { action, computed, IObservableArray, makeObservable, observable, values } from 'mobx';
import { v4 as uuidv4 } from 'uuid';

export class UiStore {
  @observable notifs: IObservableArray<NotificationModel> = observable.array<NotificationModel>([]);

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
  }

  @action
  fadeOut(uuid: string) {
    this.notifs.find((x) => x.uuid == uuid).show = false;
    setTimeout(() => {
      this.closeNotif(uuid); // clean up faded out items
    }, 5000);
  }

  @action
  private closeNotif(uuid: string) {
    this.notifs.replace(this.notifs.filter((x) => x.uuid !== uuid));
  }
}

const notificationsUiStore = new UiStore();
export { notificationsUiStore };
