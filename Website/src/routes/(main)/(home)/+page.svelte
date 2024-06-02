<script lang="ts">
  import * as Card from '$shadcn/components/ui/card/index';
  import GitHub from './icons/github.svelte';
  import Discord from './icons/discord.svelte';
  import Patreon from './icons/patreon.svelte';
  import BuyMeACoffee from './icons/buyMeACoffee.svelte';
  import LinkButton from './linkButton.svelte';
  import Acknowledgement from './acknowledgement.svelte';
  import Typography from '$lib/components/typography.svelte';

  import LukeFZ from '$lib/assets/acknowledgement/lukefz.webp';
  import Ceris from '$lib/assets/acknowledgement/ceris.webp';
  import Nano from '$lib/assets/acknowledgement/nano.webp';
  import Nightmerp from '$lib/assets/acknowledgement/nightmerp.webp';
  import Skazord from '$lib/assets/acknowledgement/skazord.webp';
  import Lati from '$lib/assets/acknowledgement/lati.webp';
  import Sockperson from '$lib/assets/acknowledgement/sockperson.webp';
  import FatesTimelines1337 from '$lib/assets/acknowledgement/fatestimelines1337.webp';
</script>

<div
  id="banner"
  class="background-image dark:testClass grid grid-cols-12 gap-4 bg-[url('/src/lib/assets/story.webp')] dark:bg-[url('/src/lib/assets/storyDark.webp')]">
  <Card.Root class="col-span-12 lg:col-span-7">
    <Card.Header>
      <Card.Title tag="h2" class="text-3xl">Welcome to Dawnshard</Card.Title>
    </Card.Header>
    <Card.Content>
      <p class="text-lg font-semibold" style:margin-bottom="0.5rem">
        Dawnshard is a server emulator project aimed at enabling continued play of Dragalia Lost.
      </p>
      <p>
        Ever since the official servers were discontinued in November 2022, Dawnshard has been in
        development as a fan-led reimplementation of the game&apos;s web backend.
      </p>
    </Card.Content>
    <Card.Footer class="flex-row flex-wrap gap-2">
      <LinkButton href="https://github.com/sapiensanatis/dawnshard" icon={GitHub}>
        Source code
      </LinkButton>
      <LinkButton href="https://discord.gg/j9zSttjjWj" icon={Discord}>Discord</LinkButton>
      <LinkButton href="https://patreon.com/dawnshard" icon={Patreon}>Patreon</LinkButton>
      <LinkButton href="https://buymeacoffee.com/dawnshard" icon={BuyMeACoffee}
        >Buy me a coffee</LinkButton>
    </Card.Footer>
  </Card.Root>
</div>

<div class="flex flex-col gap-5 p-5 md:w-[75%]">
  <div>
    <Typography typography="h2">About</Typography>
    <p>
      Dawnshard is attempting to reimplement the server infrastructure of Dragalia Lost. It aims to
      do so in a vanilla fashion, i.e. with limited changes to the original mechanics of the game.
      Though not all features are yet implemented, the game is in a playable state and it is
      possible to progress largely as normal.
    </p>
  </div>
  <div class="grid grid-cols-12 gap-4">
    <div class="col-span-12">
      <Typography typography="h2">How to play</Typography>
      <p>
        Setting up access to the server involves configuring a modified version of the original
        Dragalia Lost app to connect to this server at{' '}
        <a class="link" href="https://dawnshard.co.uk">https://dawnshard.co.uk</a>. It is possible
        to do so on both Android and iOS without jail-breaking or rooting your device. It is also
        possible to do this on an Android emulator, even though the original game could not be
        played on emulators. If you encounter any issues during the set-up process, consider joining
        the{' '}
        <a class="link" href="https://discord.gg/j9zSttjjWj">community Discord server</a>.
      </p>
    </div>
    <div class="col-span-12 md:col-span-6 lg:col-span-8">
      <Typography typography="h3">Android</Typography>
      <ol class="list-inside list-decimal px-4">
        <li>
          Ensure you have the original Dragalia Lost app installed. The Dragalipatch app works by
          taking the files from the original app and modifying them.
        </li>
        <li>
          Download the Dragalipatch app by LukeFZ from the{' '}
          <a class="link" href="https://github.com/lukefz/dragalipatch/releases/latest">
            GitHub releases page.
          </a>
        </li>
        <li>
          Enter <a class="link" href="https://dawnshard.co.uk">https://dawnshard.co.uk</a>{' '}
          into the &apos;Server Address&apos; field.
        </li>
        <li>
          Leave the CDN address field blank. You can try entering{' '}
          <a href={'https://cdn.minty.sbs'}>https://cdn.minty.sbs</a> if this is not accepted.
        </li>
        <li>Press the &apos;Patch App&apos; button in the lower right corner.</li>
        <li>
          Wait for the app to finish patching the Dragalia Lost app. Once it is done, press Install
          on the screen that follows.
        </li>
      </ol>
    </div>
    <aside class="col-span-12 flex flex-col items-center px-5 md:col-span-6 lg:col-span-4">
      <enhanced:img
        src="$lib/assets/dragalipatch.png"
        class="block w-full max-w-80 align-middle dark:hidden"
        alt="Example inputs of Dawnshard server address in Dragalipatch interface" />
      <enhanced:img
        src="$lib/assets/dragalipatchDark.png"
        class="hidden w-full max-w-80 align-middle dark:block"
        alt="Example inputs of Dawnshard server address in Dragalipatch interface" />
      <p class="mt-1 italic">Example Dragalipatch inputs</p>
    </aside>
    <div class="col-span-12">
      <Typography typography="h3">iOS</Typography>
      <p>
        For information on how to play on iOS, please see{' '}
        <a class="link" href="https://twitter.com/FloppyEarsRCute/status/1672549774870953985">
          this tweet, containing a guide on Google Docs</a
        >. At a high level, you will need to use{' '}
        <a class="link" href="https://sideloadly.io/">Sideloadly</a> to install a pre-patched IPA file
        that allows configuring the server address.
      </p>
    </div>
  </div>

  <div class="flex flex-col gap-3">
    <Typography typography="h2">Frequently asked questions</Typography>
    <div>
      <Typography typography="h3"
        >Can I link an account to save my progress across devices?</Typography>
      <p>
        Yes, if you follow the prompts to link an account in-game, you will be taken to the <a
          class="link"
          href="https://baas.lukefz.xyz">BaaS website</a
        >. This is a replacement for the Nintendo account linking system. If you create a new
        account and link it to your game&apos;s account, you will be able to access your progress
        from another device linked to the same account.
      </p>
    </div>
    <div>
      <Typography typography="h3">Do I have to start over?</Typography>
      <p>
        It is no longer possible to recover your progress from the original servers. However, you
        can import a save using the account linking system. You can upload a JSON file after logging
        into the BaaS with save data to be applied to the server. You can use a preset save file,
        such as this{' '}
        <a
          href="https://drive.google.com/drive/folders/17pR_hZtjIZ7NKBMUjtiY355FCM_-TqgO?usp=sharing"
          class="link">
          maxed out save file</a
        >, to skip parts of the game you do not want to play again.
      </p>
    </div>
    <div>
      <Typography typography="h3">What has been implemented?</Typography>
      <p>The majority of core gameplay mechanics have been implemented:</p>
      <ul class="list-inside list-disc px-4">
        <li>Completing quests</li>
        <li>The Halidom castle builder</li>
        <li>Co-op</li>
        <li>Kaleidoscape</li>
        <li>Upgrading weapons, wyrmprints, dragons</li>
      </ul>
    </div>
    <div>
      <Typography typography="h3">What is still being worked on?</Typography>
      <p>Here is a non-exhaustive list of features that are still being developed:</p>
      <ul class="list-inside list-disc px-4">
        <li>Friends and alliances</li>
        <li>Alberian Battle Royale</li>
        <li>Astral raids</li>
        <li>Endeavours: some are available and are being slowly added</li>
      </ul>
    </div>
  </div>
  <div>
    <Typography typography="h2">Acknowledgements</Typography>
    <p>
      Special thanks to the following people, who have contributed to the server source code or its
      development in other ways:
    </p>
    <ul class="ml-3 mt-1">
      <Acknowledgement name="LukeFZ" avatarSrc={LukeFZ}>
        for developing the BaaS account system, DragaliPatch, and implementing features including
        the event compendium, endeavours, and more.
      </Acknowledgement>
      <Acknowledgement name="Ceris" avatarSrc={Ceris}>
        for developing <a href="https://orchis.cherrymint.live">Orchis</a>, another server revival
        project for Dragalia Lost.
      </Acknowledgement>
      <Acknowledgement name="Nano" avatarSrc={Nano}>
        for contributing character, summoning and dragon functionality to the server
      </Acknowledgement>
      <Acknowledgement name="Nightmerp" avatarSrc={Nightmerp}>
        for developing helper, wyrmprint, and quest-related features.
      </Acknowledgement>
      <Acknowledgement name="Skazord" avatarSrc={Skazord}>
        for working on the Halidom.
      </Acknowledgement>
      <Acknowledgement name="sockperson" avatarSrc={Sockperson}>
        for implementing the Mercurial Gauntlet.
      </Acknowledgement>
      <Acknowledgement name="LatiWednesday" avatarSrc={Lati}>
        for working on character story epithets.
      </Acknowledgement>
      <Acknowledgement name="FatesTimelines1337" avatarSrc={FatesTimelines1337}>
        for adding treasure chests to the main campaign.
      </Acknowledgement>
    </ul>
  </div>
</div>

<style>
  #banner {
    padding: 3rem;
    object-position: 100% 50%;
    background-size: cover;
    background-position: 10% 10%;
    background-repeat: no-repeat;
    border-bottom: 1px solid;
    border-color: var(--divider);
  }

  li {
    margin: 5px 0;
  }
</style>
