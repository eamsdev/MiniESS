import { useEffect } from 'react';
import { Dashboard } from './Dashboard/Dashboard';
import { Layout } from './Shared/Layout';
import { todoUiStore } from './Todo/UiStore';

export const App = () => {
  useEffect(() => {
    todoUiStore.onNavigate().catch((e) => console.error('failed to fetch todos', e));
  });

  return (
    <Layout>
      <Dashboard />
    </Layout>
  );
};

export default App;
