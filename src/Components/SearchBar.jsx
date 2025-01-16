export default function Searchbar() {
    return (
      <div className="flex justify-center p-4 w-full">
        <input
          type="text"
          placeholder="Search for a job"
          class="w-1/2 max-w-lg p-3 shadow-lg border-4 border-blue-500 focus:outline-none rounded-lg"
        />
      </div>
    );
  }