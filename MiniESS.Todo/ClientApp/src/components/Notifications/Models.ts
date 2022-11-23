import { makeObservable, observable } from 'mobx';

export class NotificationModel {
  uuid: string;
  message: string;
  isSuccess: boolean;
  @observable show: boolean;

  constructor(uuid: string, message: string, isSuccess: boolean) {
    makeObservable(this);

    this.uuid = uuid;
    this.message = message;
    this.isSuccess = isSuccess;
    this.show = true;
  }
}
