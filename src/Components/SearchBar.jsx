export default function Searchbar() {
    return (
      <div className="flex justify-center p-4 w-full">
        <input
          type="text"
          placeholder="Search for a job"
          class="text-text placeholder-subtext0 bg-crust w-1/2 max-w-lg p-3 focus:outline-none rounded-lg"
        />
      </div>
    );
  }