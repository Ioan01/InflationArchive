export const environment = "dev";

export const localhostAddress = "http://localhost:30620";
export const serverAddress = "";

export const address = () =>
  environment == "dev" ? localhostAddress : serverAddress;
