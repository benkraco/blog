import { Outlet } from "react-router-dom";
import "../../style.css";

function LayoutLogin() {
  return (
    <div className="layout layoutLogin">
      <main className="loginMain">
        <Outlet />
      </main>
    </div>
  );
}

export default LayoutLogin;