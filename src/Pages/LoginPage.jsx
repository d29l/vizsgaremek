import { useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import axios from "axios";

export default function LoginPage() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [incorrect, setIncorrect] = useState(false);
  const [showPass, setShowPass] = useState(false); 

  function decodeJWT(token) {
    try {
      const [header, payload, signature] = token.split('.');

      const decodedPayload = atob(payload.replace(/-/g, '+').replace(/_/g, '/'));

      return JSON.parse(decodedPayload);
    } catch (error) {
      console.error('Failed to decode JWT:', error);
      return null;
    }
  }

  const handleLogin = async (e) => {
    e.preventDefault();

    try {
      const response = await axios.post(
        "https://localhost:7077/api/users/loginUser",
        {
          email,
          password,
        },
      );

      if (response.status == 200) {
        navigate("/");
        console.log(response.data);
        console.log("Successfully signed in");
        localStorage.setItem("token", response.data.token);
        console.log(decodeJWT(response.data.token));
      } else {
        console.log(response.data.message || "Sign in failed");
      }
    } catch (err) {
      if (err.status == 401) {
        setIncorrect(true)
      }
      console.log(
        err.response?.data?.message || "An error occurred during signing in",
      );
    }
  };

  const navigate = useNavigate();
  const handleRedirect = () => {
    navigate(`/signup`);
  };

  const handleShowPassword = (e) => {
    const checked = e.target.checked
    setShowPass(checked)
  }

  return (
    <div class="flex min-h-[92vh] items-center justify-center">
      <div class="w-[24rem] rounded-lg bg-base py-10 pt-6 shadow-md shadow-crust">
        <div class="flex justify-center">
          <h1 class="text-2xl font-extrabold text-lavender">Sign in</h1>
        </div>
        <div class="flex flex-col items-start px-8 pt-8">
          <form class="flex w-full flex-col" onSubmit={handleLogin}>
            <label class="mb-2 font-bold text-text">Email</label>
            <input
              placeholder="example@mail.com"
              class="mb-2 h-8 rounded-lg bg-mantle pl-2 text-subtext0 placeholder-surface2 focus:border-2 focus:border-lavender focus:outline-none"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
            />
            <label class="mb-2 font-bold text-text">Password</label>
            <input
              type= {showPass ? "text" : "password"}
              class="mb-2 h-8 rounded-lg bg-mantle pl-2 text-subtext0 focus:border-2 focus:border-lavender focus:outline-none"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            />
            <div class="mt-2 flex items-center">
              <input
                type="checkbox"
                class="h-4 w-4 cursor-pointer appearance-none rounded border-2 border-subtext1 transition-colors duration-200 checked:bg-lavender focus:outline-none"
                onChange={handleShowPassword}
              />
              <label class="ml-2 cursor-pointer select-none text-subtext1">
                Show password
              </label>
            </div>

            {/* <div class="mt-2 flex items-center">
              <input
                type="checkbox"
                class="h-4 w-4 cursor-pointer appearance-none rounded border-2 border-subtext1 transition-colors duration-200 checked:bg-lavender focus:outline-none"
              />
              <label class="ml-2 cursor-pointer select-none text-subtext1">
                Keep me signed in
              </label>
            </div> */}

            <div class="mt-6 flex justify-center">
              <button class="w-1/2 rounded-lg bg-lavender p-1 font-bold text-mantle">
                Sign in
              </button>
            </div>

            {incorrect && (
              <div class="text-sm font-bold text-red pt-4 self-center">Incorrect email or password</div>
            )}

            <div class="mt-6 flex flex-row justify-center">
              <p class="text-subtext1">Don't have an account yet?</p>
              <a
                class="ml-2 cursor-pointer text-lavender hover:underline"
                onClick={handleRedirect}
              >
                Sign up
              </a>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}
