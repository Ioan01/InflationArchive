<template>
    <div class="d-flex-center mt-5">
        <h3 class="text-center mt-5">Details about the following user:</h3>
        
    </div>
</template>
<script lang="ts">
    import { address } from '@/store/environment';
    import { defineComponent } from 'vue';
    import { useLoggedInUserStore } from '@/store/loggedInUser';
    import { storeToRefs } from 'pinia';

    export default defineComponent({
        setup() {

        },
        

        methods: {
            
        }
    })
</script>