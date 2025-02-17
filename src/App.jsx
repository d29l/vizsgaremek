import "./App.css";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Home from "./Pages/Home";
import PostPage from "./Pages/PostPage";
import Navbar from "./Components/Navbar";
import RegisterPage from "./Pages/RegisterPage";

function App() {
  return (
    <div class="mocha">
      <div className="bg-mantle overflow-hidden min-h-screen">
        <Navbar />
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/posts/:postId" element={<PostPage />} />
          <Route path="/register" element={<RegisterPage/>} />
        </Routes>
      </div>
    </div>
  );
}

export default App;
