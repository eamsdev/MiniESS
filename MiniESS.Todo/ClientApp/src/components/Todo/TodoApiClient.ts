import axios from 'axios';
import { notificationsUiStore } from '../Notifications/UiStore';

class AxiosFactory {
  static createAxiosInstance() {
    const client = axios.create({
      baseURL: 'todo/',
    });

    client.interceptors.response.use(
      (resp) => {
        return resp;
      },
      (error) => {
        notificationsUiStore.addNotification('Something went wrong!', false);
        return Promise.reject(error);
      },
    );

    return client;
  }
}

export default AxiosFactory.createAxiosInstance();
