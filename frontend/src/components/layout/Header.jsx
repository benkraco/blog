import "../../style.css";

function Header() {
  return (
    <header>
      <img src="../../../public/img/icon.png" alt="Logo" />

      <nav>
        <ul>
          <li>
            <a href="">Blog</a>
          </li>
          <li>
            <a href="">Portfolio</a>
          </li>
          <li>
            <a href="https://github.com/benkraco/blog" target="_blank" rel="noopener noreferrer">GitHub</a>
          </li>
        </ul>
      </nav>

      <a href="">Login</a>
    </header>
  );
}

export default Header;
