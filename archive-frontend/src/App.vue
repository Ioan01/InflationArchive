<template>
  <v-app>
    <ToolbarView></ToolbarView>

    <v-main>
      <router-view />
    </v-main>
  </v-app>
</template>

<script lang="ts">
import Vue from 'vue';
import ToolbarView from './views/ToolbarView.vue';

export default Vue.extend({
  name: "App",
  data: () => ({
    //
  }),
  components: { ToolbarView }
});
</script>
