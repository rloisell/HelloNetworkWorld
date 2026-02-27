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
import DashboardPage from './pages/DashboardPage.jsx';
import TestsPage from './pages/TestsPage.jsx';
import DocsPage from './pages/DocsPage.jsx';
import './App.css';

// ── APP ──────────────────────────────────────────────────────────────────────
export default function App() {
  return (
    <BrowserRouter>
      {/* BC Gov Design System header */}
      <BCGovHeader />

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
            Standards &amp; Docs
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

      {/* BC Gov footer */}
      <BCGovFooter />
    </BrowserRouter>
  );
} // end App

// ── BC GOV HEADER ────────────────────────────────────────────────────────────
// BC Gov standard header with logo and skip-to-content link
function BCGovHeader() {
  return (
    <header className="bcgov-header">
      <a href="#main-content" className="bcgov-skip-to-content">Skip to main content</a>
      <div className="bcgov-header__container">
        <div className="bcgov-header__logo">
          <a href="https://gov.bc.ca" aria-label="Province of British Columbia">
            <img src="/bcid-logo-rev-en.svg" alt="Province of British Columbia" width="177" height="44" />
          </a>
        </div>
        <div className="bcgov-header__title">
          <span>HelloNetworkWorld</span>
          <span className="bcgov-header__subtitle">Network Health &amp; BC Gov Standards</span>
        </div>
      </div>
    </header>
  );
} // end BCGovHeader

// ── BC GOV FOOTER ────────────────────────────────────────────────────────────
// BC Gov standard footer
function BCGovFooter() {
  return (
    <footer className="bcgov-footer">
      <div className="bcgov-footer__container">
        <ul className="bcgov-footer__links">
          <li><a href="https://www2.gov.bc.ca/gov/content/home/disclaimer">Disclaimer</a></li>
          <li><a href="https://www2.gov.bc.ca/gov/content/home/privacy">Privacy</a></li>
          <li><a href="https://www2.gov.bc.ca/gov/content/home/accessibility">Accessibility</a></li>
          <li><a href="https://www2.gov.bc.ca/gov/content/home/copyright">Copyright</a></li>
        </ul>
      </div>
    </footer>
  );
} // end BCGovFooter
