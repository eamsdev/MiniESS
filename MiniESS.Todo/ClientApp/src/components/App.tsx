import { observer } from 'mobx-react';
import { Page, pageStore } from './AppStore';
import { Dashboard } from './Dashboard/Dashboard';
import { Layout } from './Shared/Layout';
import { Login } from './Login/Login';

export const App = observer(() => {
  const pages = {
    [Page.Home]: <Dashboard />,
    [Page.Login]: <Login />,
  };

  return <Layout>{pages[pageStore.activePage]}</Layout>;
});

export default App;
