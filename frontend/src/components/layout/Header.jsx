import "../../style.css";
import { Link } from "react-router-dom";
import useAuth from "../../hooks/useAuth";
import { useState } from "react";

function Header() {
  const { isAuthenticated, logout } = useAuth();
  const [isDropdownOpen, setIsDropdownOpen] = useState(false);

  const handleLogout = async () => {
    try {
      await logout();
    } catch (error) {
      console.error("Error al cerrar sesión:", error);
    }
  };

  return (
    <header>
      <img src="/img/icon.png" alt="Logo" />

      <nav>
        <ul>
          <li>
            <Link to="/">Blog</Link>
          </li>
          <li>
            <a
              href="https://benkraco.com"
              target="_blank"
              rel="noopener noreferrer"
            >
              Portfolio
            </a>
          </li>
          <li>
            <a
              href="https://github.com/benkraco/blog"
              target="_blank"
              rel="noopener noreferrer"
            >
              GitHub
            </a>
          </li>
        </ul>
      </nav>

      {isAuthenticated ? (
        <div className="userMenu">
          <button
            type="button"
            className="userMenuButton"
            onClick={() => setIsDropdownOpen((prev) => !prev)}
          >
            Admin
          </button>

          {isDropdownOpen && (
            <div className="userDropdown">
              <Link to="/upload">Crear un posteo</Link>

              <button type="button" onClick={handleLogout}>
                Cerrar sesión
              </button>
            </div>
          )}
        </div>
      ) : (
        <Link to="/login">Login</Link>
      )}
    </header>
  );
}

export default Header;
