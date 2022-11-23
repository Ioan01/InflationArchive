import { LoginModel } from "@/models/loginModel";
import { defineStore } from "pinia";
import { address } from "./environment";

export const useGlobalStore = defineStore("globals", {
  state: () => {
    return { loggedIn: false, token: "" };
  },

  // could also be defined as
  // state: () => ({ count: 0 })
  actions: {
    logIn() {
      this.loggedIn = true;
      console.log(this.loggedIn);
    },
  },
});
