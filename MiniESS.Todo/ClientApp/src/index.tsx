import { createRoot } from 'react-dom/client';
import App from './components/App';
import './assets/scss/site.scss';

const container = document.getElementById('root');

const root = createRoot(container);

root.render(<App />);
