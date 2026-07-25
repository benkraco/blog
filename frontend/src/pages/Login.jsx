import PageTitle from "../components/ui/PageTitle";
import { useState, useEffect } from "react";
import IconUser from "../assets/IconUser.png";
import { Link, useNavigate } from "react-router-dom";
import useAuth from "../hooks/useAuth";
import ErrorMessage from "../components/ui/Error";

function Login() {
  const navigate = useNavigate();
  const { login } = useAuth();

  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");

  const [error, setError] = useState("");

  const [date, setDate] = useState(new Date());

  useEffect(() => {
    const interval = setInterval(() => {
      setDate(new Date());
    }, 1000);

    return () => clearInterval(interval);
  }, []);

  const day = date
    .toLocaleDateString("es-AR", {
      weekday: "long",
      month: "long",
      day: "numeric",
    })
    .replace(/^./, (letter) => letter.toUpperCase());

  const time = date.toLocaleTimeString("en-US", {
    hour: "2-digit",
    minute: "2-digit",
    hour12: false,
  });

  const handleSubmit = async (event) => {
    event.preventDefault();

    setError("");

    try {
      await login(username, password);

      navigate("/");
    } catch (error) {
      console.error(error);

      setError("Usuario o contraseña incorrectos.");
    }
  };

  return (
    <>
      <PageTitle title="Login" />

      <section className="loginTime">
        <p>{day}</p>
        <p>{time}</p>
      </section>

      <form className="loginForm" onSubmit={handleSubmit}>
        <img src={IconUser} alt="" />

        <div className="loginInputs">
          <p>Username:</p>
          <input
            type="text"
            value={username}
            onChange={(event) => setUsername(event.target.value)}
          />
        </div>

        <div className="loginInputs">
          <p>Password:</p>
          <input
            type="password"
            value={password}
            onChange={(event) => setPassword(event.target.value)}
          />
        </div>

        {error && <ErrorMessage message={error}/>}

        <div className="loginButtons">
          <Link to="/" className="loginCancel">
            Cancel
          </Link>
          <input type="submit" value="Login" />
        </div>
      </form>
    </>
  );
}

export default Login;
