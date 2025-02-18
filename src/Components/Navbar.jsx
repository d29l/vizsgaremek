import React from "react";

export default function Navbar() {
  return (
    <nav className="h-[8vh] bg-base pb-1">
      <div className="flex h-full items-center justify-between shadow-md shadow-crust">
        <div className="flex items-center space-x-6 pl-4">
          <div className="flex flex-col">
            <h1 className="text-2xl font-extrabold text-lavender">
              Job platform
            </h1>
          </div>

          <div className="flex space-x-4"></div>
        </div>

        <div className="mr-4 flex size-10 overflow-hidden rounded-full bg-surface0"></div>
      </div>
    </nav>
  );
}
