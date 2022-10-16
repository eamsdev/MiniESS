import { FunctionComponent } from 'react';
import styled from 'styled-components';
import { Container } from 'react-bootstrap';

const StyledFooter = styled.footer`
  position: fixed;
  left: 0;
  bottom: 0;
  width: 100%;
`;

export const Footer: FunctionComponent = () => {
  return (
    <StyledFooter className="d-flex flex-wrap justify-content-between align-items-center py-3 border-top">
      <Container className="container-fluid">
        <div className="text-center">{`Copyright © ${new Date().getFullYear()} eams.dev`}</div>
      </Container>
    </StyledFooter>
  );
};
