<template>
    <div class="d-flex-center mt-5">
        <h3 class="text-center mt-5">Details about the following user:</h3>
        <h3 class="text-center mt-5">User's favorite products:</h3>
        <v-card class="mx-auto mt-5" max-width="344">
            <v-img src="https://cdn.vuetifyjs.com/images/cards/sunshine.jpg" height="200px"></v-img>

            <v-card-title>
                Product name lalala
            </v-card-title>

            <v-card-subtitle>
                Do we really need this buddy?
            </v-card-subtitle>
            <v-row class="justify-center mx-auto pa-3">
                <v-btn
                    color="primary" >
                    Product details
                </v-btn>
            </v-row>
        </v-card>
    </div>
</template>
<script lang="ts">
    import { address } from '@/store/environment';
    import { defineComponent } from 'vue';

    import { storeToRefs } from 'pinia';

    export default defineComponent({
        setup() {

        },
        

        methods: {

        }
    })
</script>