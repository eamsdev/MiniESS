import { observer } from 'mobx-react';
import { FC } from 'react';
import { ToastContainer, Toast } from 'react-bootstrap';
import { notificationsUiStore } from './UiStore';

export const Notifications: FC = observer(() => {
  return (
    <ToastContainer className="mt-4" position="top-center">
      {notificationsUiStore.notifications.map((x) => (
        <Toast
          onClose={() => notificationsUiStore.fadeOut(x.uuid)}
          delay={5000}
          show={x.show}
          bg={x.isSuccess ? 'success' : 'danger'}
          key={x.uuid}
          autohide
        >
          <Toast.Header>
            <strong className="me-auto">{x.isSuccess ? 'Success' : 'Failed'}</strong>
          </Toast.Header>
          <Toast.Body>{x.message}</Toast.Body>
        </Toast>
      ))}
    </ToastContainer>
  );
});
