import { LoginModel } from "@/models/loginModel";
import { defineStore } from "pinia";
import { address } from "./environment";

export const useGlobalStore = defineStore("globals", {
  state: () => {
    return { token: "" };
  },

  // could also be defined as
  // state: () => ({ count: 0 })
  actions: {
    logIn(token: string) {
      this.token = token;
      console.log(this.token);
    },
  },
});
