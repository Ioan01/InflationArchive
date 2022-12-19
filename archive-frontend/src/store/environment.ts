export const environment = "dev";

export const localhostAddress = "http://localhost:5016";
export const serverAddress = "";

export const address = () =>
  environment == "dev" ? localhostAddress : serverAddress;
