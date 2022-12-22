import Vue from "vue";
import "./plugins/axios";
import App from "./App.vue";
import router from "./router";
import store from "./store";
import vuetify from "./plugins/vuetify";

Vue.config.productionTip = false;
import { createPinia, PiniaVuePlugin } from "pinia";
import axios from "axios";

Vue.use(PiniaVuePlugin);
axios.defaults.headers.post["Content-Type"] = "multipart/form-data";

const pinia = createPinia();

new Vue({
  router,
  store,
  vuetify,
  pinia,
  render: (h) => h(App),
}).$mount("#app");
