import { observer } from 'mobx-react';
import { FunctionComponent, PropsWithChildren } from 'react';
import { Container } from 'react-bootstrap';
import { Navbar as NavComponent, NavLink } from '../../components-library/Navbar';
import { TransitionWrapper } from '../../components-library/TransitionWrapper';
import { navbarConfig } from '../../config/navbar.config';
import { TRANSITION_DEFAULT } from '../../config/transition.config';
import { pageStore } from '../AppStore';
import { Footer } from '../Footer/Footer';

export const Layout: FunctionComponent<PropsWithChildren> = observer((props) => {
  return (
    <>
      <NavBar />
      <Container className="p-3">
        <TransitionWrapper
          transitionKey={pageStore.activePage.toString()}
          timeout={TRANSITION_DEFAULT.timeout}
          classNames="fade"
        >
          {props.children}
        </TransitionWrapper>
      </Container>
      <Footer />
    </>
  );
});

const NavBar = observer(() => {
  // Because the nav content is using callback for Active state,
  // listens on active page to ensure re - render on page change
  pageStore.activePage;

  const pageNavigations: NavLink[] = Object.keys(navbarConfig).map((key) => {
    return {
      label: navbarConfig[key].label,
      onClick: () => navbarConfig[key].onClick(),
      isActive: () => navbarConfig[key].isActive(),
      pullRight: navbarConfig[key].pullRight,
    };
  });

  return <NavComponent navLinks={pageNavigations} />;
});
