import { createRoot } from 'react-dom/client';
import App from './components/App';
import './assets/scss/site.scss';
import { rootStore } from './components/RootStore';

const container = document.getElementById('root');

const root = createRoot(container);
rootStore.routerStore.router.start();

root.render(<App />);
