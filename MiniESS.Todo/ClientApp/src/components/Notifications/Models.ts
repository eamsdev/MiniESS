export class NotificationModel {
  uuid: string;
  message: string;
  isSuccess: boolean;

  constructor(uuid: string, message: string, isSuccess: boolean) {
    this.uuid = uuid;
    this.message = message;
    this.isSuccess = isSuccess;
  }
}
