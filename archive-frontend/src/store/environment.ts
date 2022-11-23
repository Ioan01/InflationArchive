export const environment = "dev";

export const localhostAddress = "https://localhost:7038";
export const serverAddress = "";

export const address = () =>
  environment == "dev" ? localhostAddress : serverAddress;
