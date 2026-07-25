import Header from "./Header";
import Footer from "./Footer";
import { Outlet } from "react-router-dom";
import "../../style.css";

function Layout() {
  return (
    <div className="layout">
      <Header />

      <main className="layoutMain">
        <Outlet />
      </main>

      <Footer />
    </div>
  );
}

export default Layout;