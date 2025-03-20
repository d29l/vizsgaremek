import { useNavigate } from "react-router-dom";
import { useState, useEffect } from "react";
import axios from "axios";

export default function RegisterPage() {
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");

  const [showPass, setShowPass] = useState(false); 


  const navigate = useNavigate();

  const handleRegister = async (e) => {
    e.preventDefault();

    try {
      const response = await axios.post(
        "https://localhost:7077/api/users/registerUser",
        {
          firstName,
          lastName,
          email,
          password,
        },
      );

      if (response.status == 201) {
        navigate("/login");
        console.log(response.data);
        console.log("Successful registration");
      } else {
        console.log(response.data.message || "Registration failed");
      }
    } catch (err) {
      console.log(
        err.response?.data?.message || "An error occurred during registration",
      );
    }
  };

  const handleRedirect = () => {
    navigate(`/login`);
  };

  const handleShowPassword = (e) => {
    const checked = e.target.checked
    setShowPass(checked)
  }

  return (
    <div class="flex min-h-[92vh] items-center justify-center">
      <div class="w-[24rem] rounded-lg bg-base py-10 pt-6 shadow-md shadow-crust">
        <div class="flex justify-center">
          <h1 class="text-2xl font-extrabold text-lavender">Sign up</h1>
        </div>
        <div class="flex flex-col items-start px-8 pt-8">
          <form class="flex w-full flex-col" onSubmit={handleRegister}>
            <div class="flex flex-row">
              <div class="flex w-1/2 flex-col">
                <label class="mb-2 font-bold text-text">First Name</label>
                <input
                  placeholder="John"
                  class="mb-2 mr-2 h-8 rounded-lg bg-mantle pl-2 text-subtext0 placeholder-surface2 focus:border-2 focus:border-lavender focus:outline-none"
                  value={firstName}
                  onChange={(e) => setFirstName(e.target.value)}
                />
              </div>

              <div class="flex w-1/2 flex-col">
                <label class="mb-2 ml-2 font-bold text-text">Last Name</label>
                <input
                  placeholder="Doe"
                  class="mb-2 ml-2 h-8 rounded-lg bg-mantle pl-2 text-subtext0 placeholder-surface2 focus:border-2 focus:border-lavender focus:outline-none"
                  value={lastName}
                  onChange={(e) => setLastName(e.target.value)}
                />
              </div>
            </div>

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
            <label class="mb-2 font-bold text-text">Confirm password</label>
            <input
              type= {showPass ? "text" : "password"}
              class="mb-2 h-8 rounded-lg bg-mantle pl-2 text-subtext0 focus:border-2 focus:border-lavender focus:outline-none"
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
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

            <div class="mt-6 flex justify-center">
              <button class="w-1/2 rounded-lg bg-lavender p-1 font-bold text-mantle">
                Sign Up
              </button>
            </div>

            <div class="mt-6 flex flex-row justify-center">
              <p class="text-subtext1">Already have an account?</p>
              <a
                class="ml-2 cursor-pointer text-lavender hover:underline"
                onClick={handleRedirect}
              >
                Sign in
              </a>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}
