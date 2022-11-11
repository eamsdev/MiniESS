import { observer } from 'mobx-react';
import { Dashboard } from './Dashboard/Dashboard';
import { Layout } from './Shared/Layout';

export const App = observer(() => {
  return (
    <Layout>
      <Dashboard />
    </Layout>
  );
});

export default App;
