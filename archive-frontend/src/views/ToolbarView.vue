import { useGlobalStore } from '../store/global';
<template>
    <div>
        <v-toolbar color="" dense class="mb-5" elevation="4">
            <router-link to="/" class="router-link">
                <v-btn icon>
                    <v-icon color="primary" @click="log(token)">mdi-home</v-icon>
                </v-btn>
            </router-link>
            <v-spacer></v-spacer>
            <v-spacer></v-spacer>
            <div v-if="!token">
                <router-link to="/login" class="ml-1 mr-1 router-link">
                    <v-btn color="primary">Login</v-btn>
                </router-link>
            </div>

            <div v-if="token">
                <v-btn color="primary">Logout</v-btn>
            </div>

        </v-toolbar>
    </div>
</template>



<script lang="ts">import { useGlobalStore } from '@/store/global';
import { storeToRefs } from 'pinia';
import { defineComponent, ref } from 'vue-demi';
import router from '../router/index';

export default defineComponent({
    setup() {
        const { token } = storeToRefs(useGlobalStore())

        return {
            token,
            log(a: any) {
                console.log(a)
            }
        }
    }


})
</script>

<style>
.router-link {
    text-decoration: none;
}
</style>


