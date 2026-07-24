import "../../style.css";
import { useEffect, useState } from "react";

function Footer() {
  const [date, setDate] = useState(new Date());

  useEffect(() => {
    const interval = setInterval(() => {
      setDate(new Date());
    }, 1000);

    return () => clearInterval(interval);
  }, []);

  const day = date.toLocaleDateString("en-US", {
    weekday: "short",
  });

  const time = date.toLocaleTimeString("en-US", {
    hour: "2-digit",
    minute: "2-digit",
    hour12: false,
  });

  return (
    <footer>
      <p>{day} {time}</p>
      <p>Copyright &copy; {date.getFullYear()} Benkraco / Benjamin Kracovitz</p>
      <p>● Online</p>
    </footer>
  );
}

export default Footer;
