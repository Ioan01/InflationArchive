import Vue from "vue";
import './plugins/axios'
import App from "./App.vue";
import router from "./router";
import store from "./store";
import vuetify from "./plugins/vuetify";

Vue.config.productionTip = false;
import { createPinia, PiniaVuePlugin } from "pinia";

Vue.use(PiniaVuePlugin);
const pinia = createPinia();

new Vue({
  router,
  store,
  vuetify,
  pinia,
  render: (h) => h(App),
}).$mount("#app");
