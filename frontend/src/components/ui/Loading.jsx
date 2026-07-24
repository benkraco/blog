import { useState, useEffect } from "react";

function Loading() {
  const [dots, setDots] = useState("");

  useEffect(() => {
    const interval = setInterval(() => {
      setDots((prev) => {
        if (prev.length >= 3) {
          return "";
        }

        return prev + ".";
      });
    }, 250);

    return () => clearInterval(interval);
  }, []);

  return (
    <div className="messageContainer">
      <p>Cargando{dots}</p>
    </div>
  );
}

export default Loading;
