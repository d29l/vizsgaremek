import { useNavigate } from "react-router-dom";
import axios from 'axios';

export default function LoginPage() {

  const navigate = useNavigate()
  const handleRedirect = () => {
    navigate(`/signup`);
  };

  return (
    <div class=" min-h-[92vh] flex justify-center items-center">
      <div class="pt-6 bg-base w-[24rem] rounded-lg py-10 shadow-md shadow-crust">
        <div class="flex  justify-center">
          <h1 class="text-lavender font-extrabold text-2xl">Sign in</h1>
        </div>
        <div class="flex flex-col items-start pt-8 px-8">
          <form class="flex flex-col w-full">
            <label class="mb-2 text-text font-bold">Email</label>
            <input placeholder="example@mail.com" class="pl-2 placeholder-surface2 text-subtext0 mb-2 rounded-lg bg-mantle h-8 focus:border-2 focus:border-lavender focus:outline-none" />
            <label class="mb-2 text-text font-bold">Password</label>
            <input type="password" class="pl-2 text-subtext0 mb-2 rounded-lg bg-mantle h-8 focus:border-2 focus:border-lavender focus:outline-none" />

            <div class="flex items-center mt-2">
              <input type="checkbox" class="appearance-none w-4 h-4 border-2 rounded border-subtext1 checked:bg-lavender cursor-pointer transition-colors duration-200 focus:outline-none" />
              <label class="ml-2 text-subtext1 cursor-pointer select-none">Keep me signed in</label>
            </div>

            <div class="flex justify-center mt-6">
              <button class="p-1 bg-lavender rounded-lg w-1/2 text-mantle font-bold">Sign in</button>
            </div>

            <div class="mt-6 flex flex-row justify-center">
              <p class="text-subtext1">Don't have an account yet?</p>
              <a class="text-lavender ml-2 hover:underline cursor-pointer" onClick={handleRedirect}>Sign up</a>
            </div>

          </form>
        </div>
      </div>
    </div>
  )
}