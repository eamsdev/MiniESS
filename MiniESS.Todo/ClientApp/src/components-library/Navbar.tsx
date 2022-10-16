import { FC, PropsWithChildren } from 'react';
import logo from '../assets/img/logo.svg';

export type NavLinks = {
  navLinks: NavLink[];
};

export type NavLink = {
  label: string;
  onClick: () => void;
  isActive?: () => boolean;
  pullRight?: boolean;
};

export const Navbar: FC<NavLinks> = (props) => {
  return (
    <NavbarContainer>
      <NavbarLogo />
      <NavbarToggler />
      <NavbarCollapse {...props} />
    </NavbarContainer>
  );
};

const NavbarCollapse: FC<NavLinks> = (props) => {
  return (
    <div className="collapse navbar-collapse justify-content-between" id="navbarToggler">
      <NavbarItemsGroup navLinks={props.navLinks.filter((x) => !x.pullRight)} />
      <NavbarItemsGroup navLinks={props.navLinks.filter((x) => !!x.pullRight)} />
    </div>
  );
};

const NavbarItemsGroup: FC<NavLinks> = (props) => {
  return (
    <ul className="navbar-nav">
      {props && props.navLinks.map((item) => <NavbarItem key={item.label} {...item} />)}
    </ul>
  );
};

const NavbarItem: FC<NavLink> = (props) => {
  const { label, onClick, isActive } = props;

  return (
    <li className="nav-item">
      <a className={`nav-link ${isActive() ? 'active' : ''}`} onClick={() => onClick()}>
        {label}
      </a>
    </li>
  );
};

const NavbarToggler: FC = () => {
  return (
    <button
      className="navbar-toggler"
      type="button"
      data-toggle="collapse"
      data-target="#navbarToggler"
    >
      <span className="navbar-toggler-icon"></span>
    </button>
  );
};

const NavbarLogo: FC = () => {
  return (
    <a className="navbar-brand" href="#">
      <img src={logo} width="30" height="30" alt="" />
    </a>
  );
};

const NavbarContainer: FC<PropsWithChildren> = (props) => {
  return (
    <div className="navbar-expand-lg">
      <nav className="navbar navbar-light bg-light">
        <div className="container">{props.children && props.children}</div>
      </nav>
    </div>
  );
};
