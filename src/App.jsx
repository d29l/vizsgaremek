import "./App.css";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Home from "./Pages/Home";
import PostPage from "./Pages/PostPage";
import Navbar from "./Components/Navbar";

function App() {
  return (
    <div className="bg-gray-200 overflow-hidden min-h-screen">
      <Navbar />
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/posts/:postId" element={<PostPage />} />
      </Routes>
    </div>
  );
}

export default App;
