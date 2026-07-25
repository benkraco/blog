import "../../style.css";
import { Link } from "react-router-dom";
import useAuth from "../../hooks/useAuth";

function Header() {
  const { isAuthenticated, logout } = useAuth();

  const handleLogout = async () => {
    try {
      await logout();
    } catch (error) {
      console.error("Error al cerrar sesión:", error);
    }
  };

  return (
    <header>
      <img src="../../../public/img/icon.png" alt="Logo" />

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
        <button onClick={handleLogout}>Logout</button>
      ) : (
        <Link to="/login">Login</Link>
      )}
    </header>
  );
}

export default Header;
