import { useEffect } from 'react';
import { Dashboard } from './Dashboard/Dashboard';
import { Notifications } from './Notifications/Notifications';
import { Layout } from './Shared/Layout';
import { todoUiStore } from './Todo/UiStore';

export const App = () => {
  useEffect(() => {
    todoUiStore.onNavigate().catch((e) => console.error('failed to fetch todos', e));
  });

  return (
    <Layout>
      <Dashboard />
      <Notifications />
    </Layout>
  );
};

export default App;
