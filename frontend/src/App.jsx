import { BrowserRouter, Routes, Route } from "react-router-dom";

import Layout from "./components/layout/Layout";
import LayoutLogin from "./components/layout/LayoutLogin";

import Home from "./pages/Home";
import Post from "./pages/Post";
import Login from "./pages/Login";
import Upload from "./pages/Upload";

import RedirectHomeRoute from "./routes/RedirectHomeRoute";
import RedirectLoginRoute from './routes/RedirectLoginRoute'

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route element={<Layout />}>
          <Route path="/" element={<Home />} />
          <Route path="/post/:slug" element={<Post />} />
        </Route>

        <Route element={<LayoutLogin />}>
          <Route element={<RedirectHomeRoute />}>
            <Route path="/login" element={<Login />} />
          </Route>
        </Route>

        <Route element={<Layout />}>
          <Route element={<RedirectLoginRoute />}>
            <Route path="/upload" element={<Upload />} />
          </Route>
        </Route>
      </Routes>
    </BrowserRouter>
  );
}

export default App;
