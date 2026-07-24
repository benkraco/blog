import { useEffect } from "react";

function PageTitle({ title }) {
  useEffect(() => {
    document.title = `${title} | Blog - Benkraco`;
  }, [title]);

  return null;
}

export default PageTitle;