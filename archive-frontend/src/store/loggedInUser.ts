import { UserModel } from "@/models/UserModel";
import { defineStore } from "pinia";
import { address } from "./environment";

export const useLoggedInUserStore = defineStore("loggedInUser", {
    state: () => {
      return { loggedIn: false, token: "" };
    },
  
    // could also be defined as
    // state: () => ({ count: 0 })
    actions: {
    },
  });