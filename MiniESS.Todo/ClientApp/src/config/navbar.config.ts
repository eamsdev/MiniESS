import { NavLink } from '../components-library/Navbar';
import { pageStore } from '../components/AppStore';

const navbarConfig: NavLink[] = [
  {
    label: 'Home',
    onClick: () => pageStore.homeButtonClicked(),
    isActive: () => pageStore.isAtHome,
  },
  {
    label: 'Login',
    onClick: () => pageStore.loginButtonClicked(),
    isActive: () => pageStore.isAtLogin,
    pullRight: true,
  },
];

export { navbarConfig };
