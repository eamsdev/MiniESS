import { observer } from 'mobx-react';
import { FunctionComponent, PropsWithChildren } from 'react';
import { Container } from 'react-bootstrap';
import { Navbar } from '../../components-library/Navbar';
import { Footer } from '../Footer/Footer';

export const Layout: FunctionComponent<PropsWithChildren> = observer((props) => {
  return (
    <>
      <Navbar navLinks={[]} />
      <Container className="p-3">{props.children}</Container>
      <Footer />
    </>
  );
});
