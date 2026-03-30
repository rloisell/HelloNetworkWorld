/*
 * App.jsx — HNW.WebClient
 * Ryan Loiselle — Developer / Architect
 * GitHub Copilot — AI pair programmer / code generation
 * February 2026
 *
 * Root React component. Sets up React Router with BC Gov Design System
 * header/footer shell. Routes: / (dashboard), /tests (test config), /docs (reference hub).
 * AI-assisted: layout scaffolding; reviewed and directed by Ryan Loiselle.
 */

import { BrowserRouter, Routes, Route, NavLink } from 'react-router-dom';
import { Header, Footer, FooterLinks } from '@bcgov/design-system-react-components';
import DashboardPage from './pages/DashboardPage.jsx';
import TestsPage from './pages/TestsPage.jsx';
import DocsPage from './pages/DocsPage.jsx';
import './App.css';

// ── APP ──────────────────────────────────────────────────────────────────────
export default function App() {
  return (
    <BrowserRouter>
      {/* BC Gov Design System header — logo is built into the component */}
      <Header
        title="HelloNetworkWorld"
        skipLinks={[<a key="main" href="#main-content">Skip to main content</a>]}
      />

      {/* Navigation */}
      <nav className="hnw-nav" role="navigation" aria-label="Main navigation">
        <div className="hnw-nav__container">
          <NavLink to="/" end className={({ isActive }) => isActive ? 'hnw-nav__link hnw-nav__link--active' : 'hnw-nav__link'}>
            Dashboard
          </NavLink>
          <NavLink to="/tests" className={({ isActive }) => isActive ? 'hnw-nav__link hnw-nav__link--active' : 'hnw-nav__link'}>
            Network Tests
          </NavLink>
          <NavLink to="/docs" className={({ isActive }) => isActive ? 'hnw-nav__link hnw-nav__link--active' : 'hnw-nav__link'}>
            Network Docs
          </NavLink>
        </div>
      </nav>

      {/* Main content */}
      <main className="hnw-main" id="main-content">
        <Routes>
          <Route path="/"      element={<DashboardPage />} />
          <Route path="/tests" element={<TestsPage />} />
          <Route path="/docs"  element={<DocsPage />} />
        </Routes>
      </main>

      {/* BC Gov Design System footer */}
      <Footer
        links={[
          <FooterLinks
            key="about"
            title="BC Government"
            links={[
              <a key="disclaimer"    href="https://www2.gov.bc.ca/gov/content/home/disclaimer">Disclaimer</a>,
              <a key="privacy"       href="https://www2.gov.bc.ca/gov/content/home/privacy">Privacy</a>,
              <a key="accessibility" href="https://www2.gov.bc.ca/gov/content/home/accessibility">Accessibility</a>,
              <a key="copyright"     href="https://www2.gov.bc.ca/gov/content/home/copyright">Copyright</a>,
            ]}
          />,
        ]}
      />
    </BrowserRouter>
  );
} // end App
