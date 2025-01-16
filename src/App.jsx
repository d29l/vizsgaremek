import "./App.css";
import Searchbar from "./Components/Searchbar";
import PostCard from "./Components/PostCard";

function App() {
  return (
    <body class="bg-gray-200 overflow-hidden">
      <nav class="bg-white shadow-none p-4 h-[8vh]">
        <div class="flex items-center h-full">
          <h1 class="text-blue-500 font-extrabold text-2xl">Job platform</h1>
        </div>
      </nav>

      <div class="w-full bg-white shadow-md min-h-[92vh] transition-all duration-200 ease-in-out focus-within:min-h-1 flex flex-col items-center justify-center space-y-4">
        <h1 class="font-extrabold text-blue-500 text-xl">Job platform</h1>
        <Searchbar></Searchbar>
      </div>
      <div class="p-6 h-[78vh]">
        <div class= "grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-3 xl:grid-cols-3 gap-6">
          <PostCard description={"fasz"}></PostCard>
          <PostCard description={"Lorem ipsum dolor sit amet consectetur adipisicing elit. Praesentium numquam dolorem, ipsam architecto odio similique rerum eaque iste molestias aliquid nisi provident. Id, voluptatum. Eaque culpa nostrum sunt architecto harum."}></PostCard>
          <PostCard description={"AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA"}></PostCard>


        </div>
      </div>
    </body>
  );
}

export default App;